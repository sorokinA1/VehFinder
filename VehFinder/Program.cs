var vehFinderApp = new VehFinderApp(
    new DataManipulator(new VehiclesRepository(
        new StringsTextualRepository()), new IteratorHelper()));

bool isSearchById = false;
// var searchParams = new List<string> { "usa", "", "" };


// var country = vehFinderApp.GetComboBoxOptions(DataField.VehCountryField);
// var level = vehFinderApp.GetComboBoxOptions(DataField.VehLevelField);
// var type = vehFinderApp.GetComboBoxOptions(DataField.VehTypeField);


// test
var repo = new DataManipulator(new VehiclesRepository
    (new StringsTextualRepository()), new IteratorHelper());
var y = repo.FindItem(new List<string> { "usa", "MT", "10" });
foreach (var x in y)
{
    Console.WriteLine(x);
}
// var x = repo.FindItem("Lat", isSearchById);
// var y = repo.FindItem(DataField.VehLevelField);
// foreach (var s in y)
// {
//     Console.WriteLine(s);
// }


void ClickQuickSearchSimulator()
{
}

// void ClickQuickChangeFirstSearchParam()
// {
//     searchParams[0] = "UDES";
// }


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
    CountryAndType = 0b0011,
    CountryAndLevel = 0b0101,
    TypeAndLevel = 0b0110,
    CountryAndLevelAndType = 0b0111
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
    private readonly IteratorHelper _iteratorHelper;

    public DataManipulator(IVehRepository vehRepo, IteratorHelper iteratorHelper)
    {
        _iteratorHelper = iteratorHelper;
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
        var searchParameter = (int)SearchParams.NoParams;
        if (searchParams[0] != "") searchParameter |= 1;
        if (searchParams[1] != "") searchParameter |= 1 << 1;
        if (searchParams[2] != "") searchParameter |= 1 << 2;
        // Console.WriteLine(Convert.ToString(searchParameter, 2));

        var itemsFound = new List<string>();
        switch (searchParameter)
        {
            // [0] country, [1] type, [2] level
            case (int)SearchParams.NoParams:
                return _iteratorHelper.Iterate(_vehicleCollection);
            case (int)SearchParams.Country:
                Console.WriteLine("Show VehCountry");
                return _iteratorHelper.Iterate(_vehicleCollection, searchParams, DataField.VehCountryField);
            case (int)SearchParams.Type:
                Console.WriteLine("Show VehType");
                return _iteratorHelper.Iterate(_vehicleCollection, searchParams, DataField.VehTypeField);
            case (int)SearchParams.Level:
                Console.WriteLine("Show VehLevel");
                return _iteratorHelper.Iterate(_vehicleCollection, searchParams, DataField.VehLevelField);
            case (int)SearchParams.CountryAndType:
                Console.WriteLine("Show VehCountry and VehType");
                return _iteratorHelper.Iterate(_vehicleCollection, searchParams, DataField.VehCountryField,
                    DataField.VehTypeField);
            case (int)SearchParams.CountryAndLevel:
                Console.WriteLine("Show vehicle with country and level");
                break;
            case (int)SearchParams.TypeAndLevel:
                Console.WriteLine("Show vehicle with type and level");
                break;
            case (int)SearchParams.CountryAndLevelAndType:
                Console.WriteLine("Show vehicle with country and type and level");
                break;
        }


        return itemsFound;
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

public class IteratorHelper
{
    public List<string> Iterate(List<string[]> collection, List<string> itemName, params DataField[] dataField)
    {
        var itemsFound = new List<string>();
        var paramsCount = dataField.Length;

        Console.WriteLine($"ItemName[0]: {itemName[0]}");
        Console.WriteLine($"Datafield[0]: {dataField[0]}");
        Console.WriteLine($"ItemName[1]: {itemName[1]}");
        Console.WriteLine($"Datafield[1]: {dataField[1]}");

        foreach (var item in collection)
        {
            switch (paramsCount)
            {
                case 1:
                {
                    if (item[(int)dataField[0]].Contains(itemName[0]))
                    {
                        itemsFound.Add(string.Join(", ", item));
                    }

                    break;
                }
                case 2:
                {
                    if (item[(int)dataField[0]].Contains(itemName[0]) &&
                        item[(int)dataField[1]].Contains(itemName[1]))
                    {
                        itemsFound.Add(string.Join(", ", item));
                    }

                    break;
                }
                case 3:
                {
                    if (item[(int)dataField[0]].Contains(itemName[0]) &&
                        item[(int)dataField[1]].Contains(itemName[1]) &&
                        item[(int)dataField[2]].Contains(itemName[2]))
                    {
                        itemsFound.Add(string.Join(", ", item));
                    }

                    break;
                }
                default:
                    itemsFound.Add(string.Join(", ", item));
                    break;
            }
        }

        return itemsFound;
    }

    public List<string> Iterate(List<string[]> collection)
    {
        var itemsFound = new List<string>();

        foreach (var item in collection)
        {
            itemsFound.Add(string.Join(", ", item));
        }

        return itemsFound;
    }
}

// if ((numbers in array == 1))
// if ((numbers in array == 2))
// if ((numbers in array == 3))
// {
//     sum += number;
// }