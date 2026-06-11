using Gridfall.Domain;

namespace Gridfall.Contracts;

public interface ISaveService
{
	void Save(SaveData saveData);
	SaveData Load();
	bool SaveExists();
}