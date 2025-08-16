using Avalonia.Reactive;

namespace Avalonia.Media;

public sealed class MatrixTransform : Transform
{
	public static readonly StyledProperty<Matrix> MatrixProperty = AvaloniaProperty.Register<MatrixTransform, Matrix>("Matrix", Matrix.Identity);

	public Matrix Matrix
	{
		get
		{
			return GetValue(MatrixProperty);
		}
		set
		{
			SetValue(MatrixProperty, value);
		}
	}

	public override Matrix Value => Matrix;

	public MatrixTransform()
	{
		this.GetObservable(MatrixProperty).Subscribe(delegate
		{
			RaiseChanged();
		});
	}

	public MatrixTransform(Matrix matrix)
		: this()
	{
		Matrix = matrix;
	}
}
