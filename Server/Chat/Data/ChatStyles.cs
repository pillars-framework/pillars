namespace Pillars.Chat.Data;

/// <summary>
/// Collection of all chat styles
/// </summary>
public static class ChatStyles
{
	public static readonly ChatStyle Default = new(CHATTEXTSTYLE.DEFAULT, "Default");
	public static readonly ChatStyle Gryffindor = new(CHATTEXTSTYLE.GRYFFINDOR, "Gryffindor");
	public static readonly ChatStyle Hufflepuff = new(CHATTEXTSTYLE.HUFFLEPUFF, "Hufflepuff");
	public static readonly ChatStyle Ravenclaw = new(CHATTEXTSTYLE.RAVENCLAW, "Ravenclaw");
	public static readonly ChatStyle Slytherin = new(CHATTEXTSTYLE.SLYTHERIN, "Slytherin");
	public static readonly ChatStyle Admin = new(CHATTEXTSTYLE.ADMIN, "Admin");
	public static readonly ChatStyle Dev = new(CHATTEXTSTYLE.DEV, "Dev");
	public static readonly ChatStyle Server = new(CHATTEXTSTYLE.SERVER, "Server");
	public static readonly ChatStyle Red = new(CHATTEXTSTYLE.RED, "FF0000FF");
	public static readonly ChatStyle Blue = new(CHATTEXTSTYLE.BLUE, "0000FFFF");
	public static readonly ChatStyle Green = new(CHATTEXTSTYLE.GREEN, "00FF00FF");
	public static readonly ChatStyle Yellow = new(CHATTEXTSTYLE.YELLOW, "FFFF00FF");
	public static readonly ChatStyle Magenta = new(CHATTEXTSTYLE.MAGENTA, "FF00FFFF");
	public static readonly ChatStyle Cyan = new(CHATTEXTSTYLE.CYAN, "00FFFFFF");

	public static readonly Dictionary<CHATTEXTSTYLE, ChatStyle> Styles = new()
	{
		{ CHATTEXTSTYLE.DEFAULT, Default },
		{ CHATTEXTSTYLE.GRYFFINDOR, Gryffindor },
		{ CHATTEXTSTYLE.HUFFLEPUFF, Hufflepuff },
		{ CHATTEXTSTYLE.RAVENCLAW, Ravenclaw },
		{ CHATTEXTSTYLE.SLYTHERIN, Slytherin },
		{ CHATTEXTSTYLE.ADMIN, Admin },
		{ CHATTEXTSTYLE.DEV, Dev },
		{ CHATTEXTSTYLE.SERVER, Server },
		{ CHATTEXTSTYLE.RED, Red },
		{ CHATTEXTSTYLE.BLUE, Blue },
		{ CHATTEXTSTYLE.GREEN, Green },
		{ CHATTEXTSTYLE.YELLOW, Yellow },
		{ CHATTEXTSTYLE.MAGENTA, Magenta },
		{ CHATTEXTSTYLE.CYAN, Cyan },
	};
}
