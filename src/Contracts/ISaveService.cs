using Gridfall.Domain;
using Gridfall.Characters.Domain;

namespace Gridfall.Contracts;

public interface ISaveService
{
	void Save(CharacterBase character);
	SaveData Load();
	bool SaveExists();
}
