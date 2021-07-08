﻿using System;

namespace CustomerLib.Business.Entities
{
	[Serializable]
	public abstract class Person : Entity
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
