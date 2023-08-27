var vehFinderApp = new VehFinderApp(new VehiclesRepository(new StringsTextualRepository()));

bool isSearchById = false;


void ClickQuickSearchSimulator()
{
    var searchBoxValue = "Udes";
    Console.WriteLine(vehFinderApp.FindOneVehicle(searchBoxValue, isSearchById));
}


public interface IVehFinderApp
{
    List<string> FindOneVehicle(string vehicleName, bool isSearchById);
}


public class VehFinderApp : IVehFinderApp
{
    private readonly Dictionary<string, List<string>> _vehicleCollection;

    public VehFinderApp(IVehRepository vehRepo)
    {
        _vehicleCollection = vehRepo.CreateVehicleCollection(vehRepo.Read("vehicles.txt"));
    }

    public List<string> FindOneVehicle(string vehicleName, bool isSearchById)
    {
        var allVehiclesFound = new List<List<string>>();
        var placeToSearch = isSearchById ? _vehicleCollection : _vehicleCollection.Values;
    }
}

public interface IVehRepository
{
    List<string> Read(string filePath);
    Dictionary<string, List<string>> CreateVehicleCollection(List<string> vehicleList);
}

public class VehiclesRepository : IVehRepository
{
    private readonly IStringsRepository _stringsRepository;


    public VehiclesRepository(IStringsRepository stringsRepository)
    {
        _stringsRepository = stringsRepository;
    }


    public List<string> Read(string filePath)
    {
        List<string> VehiclesFromFile = _stringsRepository.Read(filePath);
        var vehicles = new List<string>();

        foreach (var vehicle in VehiclesFromFile)
        {
            vehicles.Add(vehicle);
        }

        return vehicles;
    }

    public Dictionary<string, List<string>> CreateVehicleCollection(List<string> vehicleList)
    {
        var vehicleCollection = new Dictionary<string, List<string>>();

        foreach (var vehicle in vehicleList)
        {
            var extractedInfo = vehicle.Split("|");
            var vehicleInfo = new List<string>();

            for (int i = 1; i < extractedInfo.Length; i++)
            {
                vehicleInfo.Add(extractedInfo[i]);
            }

            vehicleCollection.Add(extractedInfo[0], vehicleInfo);
        }

        return vehicleCollection;
    }
}

public interface IStringsRepository
{
    List<string> Read(string filePath);
}

public abstract class StringsRepository : IStringsRepository
{
    public List<string> Read(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException("File vehicles.txt is not found");
        var fileContents = File.ReadAllText(filePath);
        return TextToStrings(fileContents);
    }

    protected abstract List<string> TextToStrings(string fileContents);
}

public class StringsTextualRepository : StringsRepository
{
    private static readonly string Separator = Environment.NewLine;

    protected override List<string> TextToStrings(string fileContents)
    {
        return fileContents.Split(Separator).ToList();
    }
}