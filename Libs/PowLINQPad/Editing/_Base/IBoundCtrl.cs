namespace PowLINQPad.Editing._Base;

public interface IBoundCtrl<T> : IDisp
{
	IRwBndVar<T> RxVar { get; }
}