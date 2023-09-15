<Query Kind="Program">
  <Namespace>PowLINQPad.RxControls</Namespace>
  <Namespace>PowRxVar</Namespace>
  <Namespace>LINQPad.Controls</Namespace>
  <Namespace>PowLINQPad.UtilsUI</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>PowLINQPad.Structs</Namespace>
  <Namespace>PowBasics.CollectionsExt</Namespace>
</Query>

#load ".\libs\base"

enum Tab
{
	Int,
	BoolOpt,
	Text,
	TextRegex,
	IntSlider,
	RngInt,
	RngTime,
	EnumSingle,
	EnumMultiple,
}

void Main()
{
	var tab = Var.Make(Tab.Int).D(D);
	
	var serD = new SerialDisp<Disp>().D(D);
	
	var times = Enumerable.Range(0, 60).SelectToArray(e => new DateTime(2023, 8, 1) + TimeSpan.FromDays(e));
	
	new Div(
	
		new Div(
			Ctrls.MkTabber(tab, (e, isOn) => new Span($"{e}").stTabBtn(isOn).D(D)).D(D)
		).stHorz(),
		
		tab
			.SelectSer(t => t switch
			{
			
				Tab.Int				=> demo<int>		($"{t}", rxVar => Ctrls.MkIntSlider		(rxVar, opt("val"),				bnd(0, 50, 5), e => $"{e}"	), 15, 10, 35),
				Tab.BoolOpt			=> demo<BoolOpt>	($"{t}", rxVar => Ctrls.MkBoolOpt		(rxVar, opt("boolopt")										), BoolOpt.True, BoolOpt.None, BoolOpt.False),
				Tab.Text			=> demo<TxtSearch>	($"{t}", rxVar => Ctrls.MkText			(rxVar, opt("text"),			false						), new TxtSearch(false, "abc"), new TxtSearch(false, "def"), new TxtSearch(false, "ghi")),
				Tab.TextRegex		=> demo<TxtSearch>	($"{t}", rxVar => Ctrls.MkText			(rxVar, opt("text"),			true						), new TxtSearch(false, "abc"), new TxtSearch(false, "def"), new TxtSearch(false, "ghi")),
				Tab.IntSlider		=> demo<int>		($"{t}", rxVar => Ctrls.MkIntSlider		(rxVar, opt("intslider"),		bnd(0, 60, 5), e => $"{e}"	), 20, 10, 35),
				Tab.RngInt			=> demo<RngInt>		($"{t}", rxVar => Ctrls.MkRngInt		(rxVar, opt("rngint"),			bnd(0, 60, 5)				), new RngInt(10, 25), new RngInt(35, 40), new RngInt(15, 30)),
				Tab.RngTime			=> demo<RngTime>	($"{t}", rxVar => Ctrls.MkRngTime		(rxVar, opt("rngtime"),			times, 60					), new RngTime(new DateTime(2023, 8, 25), new DateTime(2023, 9, 10)), new RngTime(new DateTime(2023, 7, 4), new DateTime(2023, 8, 3)), new RngTime(new DateTime(2023, 9, 12), new DateTime(2023, 9, 24))),
				Tab.EnumSingle		=> demo<Tab>		($"{t}", rxVar => Ctrls.MkEnumSingle	(rxVar, opt("enumsingle")									), Tab.IntSlider, Tab.Text, Tab.BoolOpt),
				Tab.EnumMultiple	=> demo<Tab[]>		($"{t}", rxVar => Ctrls.MkEnumMultiple	(rxVar, opt("enummultiple")									), new Tab[] { Tab.EnumMultiple, Tab.Text, Tab.RngInt }, new Tab[] { Tab.BoolOpt, Tab.RngTime }, new Tab[] { }),
				
				_ => throw new ArgumentException()
				
			}).D(D)
			.ToDC(D)
	
	)
	.stPage()
	.Dump();
}




static class CssExt
{
	private const string PaneHeaderTabBtnOn = "#3F99E2";
	private const string PaneHeaderTabBtnOff = "#404EE5";
	
	public static Control stPage(this Control c) => c.Css("""
		display: flex;
		flex-direction: column;
		row-gap: 10px;
	""");

	public static Control stHorz(this Control c) => c.Css("""
		display: flex;
		align-items: center;
		column-gap: 10px;
	""");
	
	public static (Control, IDisposable) stTabBtn(this Control c, IRoVar<bool> isOn) => c
		.Css("""
			padding:	10px 20px;
			cursor:		pointer;
		""")
		.React(isOn, v => c.Styles["background-color"] = v ? PaneHeaderTabBtnOn.v() : PaneHeaderTabBtnOff.v());

	private static (Control, IDisposable) React<T>(this Control c, IRoVar<T> rxVar, Action<T> action) => rxVar.Subscribe(action).WithObj(c);
	private static (T, IDisposable) WithObj<T>(this IDisposable d, T obj) => (obj, d);
}



static class RxExt
{
	public static (IObservable<U>, IDisposable) SelectSer<T, U>(this IObservable<T> obs, Func<T, (U, IDisposable)> fun)
	{
		var d = new Disp();
		var serD = new SerialDisp<Disp>().D(d);
		return (
			obs.Select(t =>
			{
				serD.Value = null;
				serD.Value = new Disp();
				var u = fun(t).D(serD.Value);
				return u;
			}),
			d
		);
	}
}



