using System;

namespace Game
{
	public interface IBreakable
	{
		Action<IBreakable> OnBreak { get; set; }
	
		bool IsBroken { get; set; }
		void Break();
	}
}