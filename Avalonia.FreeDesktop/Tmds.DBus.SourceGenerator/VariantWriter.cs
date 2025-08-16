using System;
using System.Text;
using Tmds.DBus.Protocol;

namespace Tmds.DBus.SourceGenerator;

internal static class VariantWriter
{
	public static void WriteDBusVariant(this ref MessageWriter writer, DBusVariantItem value)
	{
		writer.WriteSignature(Encoding.UTF8.GetBytes(value.Signature).AsSpan());
		writer.WriteDBusItem(value.Value);
	}

	public static void WriteDBusItem(this ref MessageWriter writer, DBusItem value)
	{
		if (!(value is DBusVariantItem value2))
		{
			if (!(value is DBusByteItem dBusByteItem))
			{
				if (!(value is DBusBoolItem dBusBoolItem))
				{
					if (!(value is DBusInt16Item dBusInt16Item))
					{
						if (!(value is DBusUInt16Item dBusUInt16Item))
						{
							if (!(value is DBusInt32Item dBusInt32Item))
							{
								if (!(value is DBusUInt32Item dBusUInt32Item))
								{
									if (!(value is DBusInt64Item dBusInt64Item))
									{
										if (!(value is DBusUInt64Item dBusUInt64Item))
										{
											if (!(value is DBusDoubleItem dBusDoubleItem))
											{
												if (!(value is DBusStringItem dBusStringItem))
												{
													if (!(value is DBusObjectPathItem dBusObjectPathItem))
													{
														if (!(value is DBusSignatureItem dBusSignatureItem))
														{
															if (!(value is DBusArrayItem dBusArrayItem))
															{
																if (!(value is DBusDictEntryItem dBusDictEntryItem))
																{
																	if (!(value is DBusStructItem dBusStructItem))
																	{
																		return;
																	}
																	writer.WriteStructureStart();
																	{
																		foreach (DBusItem item in dBusStructItem)
																		{
																			writer.WriteDBusItem(item);
																		}
																		return;
																	}
																}
																writer.WriteStructureStart();
																writer.WriteDBusItem(dBusDictEntryItem.Key);
																writer.WriteDBusItem(dBusDictEntryItem.Value);
																return;
															}
															ArrayStart start = writer.WriteArrayStart(dBusArrayItem.ArrayType);
															foreach (DBusItem item2 in dBusArrayItem)
															{
																writer.WriteDBusItem(item2);
															}
															writer.WriteArrayEnd(start);
														}
														else
														{
															writer.WriteSignature(dBusSignatureItem.Value.ToString());
														}
													}
													else
													{
														writer.WriteObjectPath(dBusObjectPathItem.Value);
													}
												}
												else
												{
													writer.WriteString(dBusStringItem.Value);
												}
											}
											else
											{
												writer.WriteDouble(dBusDoubleItem.Value);
											}
										}
										else
										{
											writer.WriteUInt64(dBusUInt64Item.Value);
										}
									}
									else
									{
										writer.WriteInt64(dBusInt64Item.Value);
									}
								}
								else
								{
									writer.WriteUInt32(dBusUInt32Item.Value);
								}
							}
							else
							{
								writer.WriteInt32(dBusInt32Item.Value);
							}
						}
						else
						{
							writer.WriteUInt16(dBusUInt16Item.Value);
						}
					}
					else
					{
						writer.WriteInt16(dBusInt16Item.Value);
					}
				}
				else
				{
					writer.WriteBool(dBusBoolItem.Value);
				}
			}
			else
			{
				writer.WriteByte(dBusByteItem.Value);
			}
		}
		else
		{
			writer.WriteDBusVariant(value2);
		}
	}
}
