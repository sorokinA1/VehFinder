var vehFinderApp = new VehFinderApp(
    new DataManipulator(new VehiclesRepository(new StringsTextualRepository())));

bool isSearchById = false;
var searchParams = new List<string> { "val", "val", "" };


// var country = vehFinderApp.GetComboBoxOptions(DataField.VehCountryField);
// var level = vehFinderApp.GetComboBoxOptions(DataField.VehLevelField);
// var type = vehFinderApp.GetComboBoxOptions(DataField.VehTypeField);


// test
var repo = new DataManipulator(new VehiclesRepository(new StringsTextualRepository()));
// var x = repo.FindItem("Lat", isSearchById);
var y = repo.FindItem(searchParams);

// var y = repo.FindItem(DataField.VehLevelField);
// foreach (var s in y)
// {
//     Console.WriteLine(s);
// }


void ClickQuickSearchSimulator()
{
}

void ClickQuickChangeFirstSearchParam()
{
    searchParams[0] = "UDES";
}


public class VehFinderApp
{
    // public List<string> Country => _dataManipulator.FindItem(DataField.VehCountryField);
    // public List<string> Level => _dataManipulator.FindItem(DataField.VehLevelField);
    // public List<string> Type => _dataManipulator.FindItem(DataField.VehTypeField);

    private readonly IDataManipulator _dataManipulator;

    public VehFinderApp(IDataManipulator dataManipulator)
    {
        _dataManipulator = dataManipulator;
    }

    // TODO may be
    public List<string> GetComboBoxOptions(DataField dataField)
    {
        return _dataManipulator.FindItem(dataField);
    }
}

public enum SearchParams
{
    NoParams = 0b0000,
    Country = 0b0001,
    Type = 0b0010,
    Level = 0b0100,
}

public enum DataField
{
    VehNameField,
    VehCountryField,
    VehTypeField,
    VehLevelField,
    VehIdField,
    VehLabelField
}

public interface IDataManipulator
{
    List<string> FindItem(string vehicleName, bool isSearchById);
    List<string> FindItem(DataField dataField);
    List<string> FindItem(List<string> searchParams);
}

public class DataManipulator : IDataManipulator
{
    private readonly List<string[]> _vehicleCollection;

    public DataManipulator(IVehRepository vehRepo)
    {
        _vehicleCollection = vehRepo.CreateVehicleCollection(vehRepo.Read("vehicles.txt"));
    }

    public List<string> FindItem(string vehicleName, bool isSearchById)
    {
        var vehiclesFound = new List<string>();
        var placeToSearch = isSearchById ? DataField.VehIdField : DataField.VehNameField;

        foreach (var vehicle in _vehicleCollection)
        {
            if (vehicle[(int)placeToSearch].Contains(vehicleName))
            {
                vehiclesFound.Add(string.Join(", ", vehicle));
            }
        }

        return vehiclesFound;
    }

    public List<string> FindItem(DataField dataField)
    {
        var allUniqueValues = new HashSet<string>();

        foreach (var value in _vehicleCollection)
        {
            allUniqueValues.Add(value[(int)dataField]);
        }

        return allUniqueValues.ToList();
    }

    public List<string> FindItem(List<string> searchParams)
    {
        var searchParameter = 0b0000;
        if (searchParams[0] != "") searchParameter |= 1 << 1;
        Console.WriteLine(Convert.ToString(searchParameter, 2));
        if (searchParams[1] != "") searchParameter |= 1 << 2;
        Console.WriteLine(Convert.ToString(searchParameter, 2));


        return new List<string>();
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
        List<string> vehiclesFromFile = _stringsRepository.Read(filePath);
        var vehicles = new List<string>();

        foreach (var vehicle in vehiclesFromFile)
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