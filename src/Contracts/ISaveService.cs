using Gridfall.Domain;
using Gridfall.Characters.Domain;

namespace Gridfall.Contracts;

public interface ISaveService
{
	void Save(CharacterBase character, int slot);
	SaveData Load(int slot);
	void DeleteSave(int slot);
	bool SaveExists(int slot);
}
