using System.Collections.Generic;
using Gridfall.Domain;
using Gridfall.Characters.Domain;

namespace Gridfall.Contracts;

public interface ISaveService
{
	void Save(List<CharacterBase> characters, int slot);
	SaveData Load(int slot);
	void DeleteSave(int slot);
	bool SaveExists(int slot);
}
