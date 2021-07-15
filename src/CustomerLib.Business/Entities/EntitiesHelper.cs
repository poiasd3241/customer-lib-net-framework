using System.Collections.Generic;
using System.Linq;

namespace CustomerLib.Business.Entities
{
	public class EntitiesHelper
	{
		public static bool ListsEqualByValues<T>(IEnumerable<T> list1, IEnumerable<T> list2)
			where T : Entity
		{
			if (list1 is null || list2 is null)
			{
				return list1 is null && list2 is null;
			}

			var count = list1.Count();

			if (count != list2.Count())
			{
				return false;
			}

			T item1, item2;

			for (int i = 0; i < count; i++)
			{
				item1 = list1.ElementAt(i);
				item2 = list2.ElementAt(i);

				if (item1 is null)
				{
					if (item2 is null)
					{
						continue;
					}

					return false;
				}

				if (item1.EqualsByValue(item2) == false)
				{
					return false;
				}
			}

			return true;
		}
	}
}
