using Gridfall.Domain;

namespace Gridfall.Contracts;

public interface ISaveService
{
	void Save(CharacterBase character);
	SaveData Load();
	bool SaveExists();
}
