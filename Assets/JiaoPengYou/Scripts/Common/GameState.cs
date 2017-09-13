namespace Lang
{
	/// <summary>
	/// 玩家座位顺序：下：0 右：1 上：2 左：3，其中下一定为自己
	/// </summary>
	public enum Seat
	{
	    /// <summary>
	    /// 下，一定为自己
	    /// </summary>
	    Down = 0,
	    Right = 1,
	    Up = 2,
	    Left = 3
	}
	
	public enum ResultType
	{
	    Victory = 0,
	    Fail = 1,
	    Dogfall = 2
	}
	
	public enum RankType
	{
	    None = -1,
	    Up = 0,
	    Middle = 1,
	    Down = 2
	}
}