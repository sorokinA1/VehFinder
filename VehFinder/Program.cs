var vehFinderApp = new VehFinderApp(new VehiclesRepository(new StringsTextualRepository()));

bool isSearchById = true;


// test
// var repo = new VehiclesRepository(new StringsTextualRepository());
// repo.CreateVehicleCollection(repo.Read("vehicles.txt"));
var repo = new VehFinderApp(new VehiclesRepository(new StringsTextualRepository()));
var x = repo.FindOneVehicle("54913", isSearchById);
foreach (var s in x)
{
    Console.WriteLine(s);
}


void ClickQuickSearchSimulator()
{
    var searchBoxValue = "UDES";
    Console.WriteLine(vehFinderApp.FindOneVehicle(searchBoxValue, isSearchById));
}


public interface IVehFinderApp
{
    List<string> FindOneVehicle(string vehicleName, bool isSearchById);
}


public class VehFinderApp : IVehFinderApp
{
    private readonly List<string[]> _vehicleCollection;

    public VehFinderApp(IVehRepository vehRepo)
    {
        _vehicleCollection = vehRepo.CreateVehicleCollection(vehRepo.Read("vehicles.txt"));
    }

    public List<string> FindOneVehicle(string vehicleName, bool isSearchById)
    {
        const int VehNameField = 0;
        const int VehIdField = 4;
        var vehiclesFound = new List<string>();
        var placeToSearch = isSearchById ? VehIdField : VehNameField;

        foreach (var vehicle in _vehicleCollection)
        {
            if (vehicle[placeToSearch].Contains(vehicleName))
            {
                vehiclesFound.Add(string.Join(", ", vehicle));
            }
        }

        return vehiclesFound;
    }
}

public interface IVehRepository
{
    List<string> Read(string filePath);
    List<string[]> CreateVehicleCollection(List<string> vehicleList);
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

    public List<string[]> CreateVehicleCollection(List<string> vehicleList)
    {
        var vehicleCollection = new List<string[]>();

        foreach (var vehicle in vehicleList)
        {
            var extractedInfo = vehicle.Split("|");
            vehicleCollection.Add(extractedInfo);
        }

        // foreach (var vehicle in vehicleList)
        // {
        //     var extractedInfo = vehicle.Split("|");
        //     var vehicleInfo = new List<string>();
        //
        //     for (int i = 1; i < extractedInfo.Length; i++)
        //     {
        //         vehicleInfo.Add(extractedInfo[i]);
        //     }
        //
        //     vehicleCollection.Add(extractedInfo[0], vehicleInfo);
        // }

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