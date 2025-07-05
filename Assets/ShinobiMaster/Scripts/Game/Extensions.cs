namespace Game
{
	public static class Extensions
	{
		public static int GetRandomWithExclusion(this System.Random rnd, int start, int end, int[] exclude) {
			var random = start + rnd.Next(end - start + 1 - exclude.Length);
			foreach (var ex in exclude) 
			{
				if (random < ex) 
				{
					break;
				}
				random++;
			}
			return random;
		}
	}
}