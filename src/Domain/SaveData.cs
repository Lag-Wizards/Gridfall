using System;
using System.Collections.Generic;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain
{
	// DTO used for persisting and loading simple character save data.
	public class SaveData
	{
		public List<CharacterSaveData> Characters { get; set; } = new();
	}
}
