namespace Project;

public partial class BaseUnit
{
	protected float Unwrap(UnitValue value) => value.GetValue(this);
}

public partial class BaseCast
{
	protected float Unwrap(UnitValue value) => value.GetValue(Parent);
}

public partial class BaseBuff
{
	protected float Unwrap(UnitValue value) => value.GetValue(Parent);
}