using System;

namespace CustomerLib.Business.Entities
{
	[Serializable]
	public abstract class Entity
	{
		public abstract bool EqualsByValue(object obj);
		protected void EnsureSameEntityType(object obj)
		{
			if (obj.GetType() != GetType())
			{
				throw new ArgumentException("Must use the same entity type for comparison");
			}
		}
	}
}
