var vehFinderApp = new VehFinderApp(
    new DataManipulator(new VehiclesRepository(
        new StringsTextualRepository()), new IteratorHelper()));

var placeToSearch = DataField.VehNameField;
// var searchParams = new List<string> { "usa", "", "" };
// Ho_Ri_3, japan, AT-SPG, 10, 24096

var controller = new DataManipulator(new VehiclesRepository(new StringsTextualRepository()), new IteratorHelper());
// var x = controller.FindItem("Ho", DataField.VehNameField);

var x = controller.FindUniqueItems(DataField.VehCountryField);
Console.WriteLine(x);

foreach (var s in x)
{
    Console.WriteLine(s);
}


// var country = vehFinderApp.GetComboBoxOptions(DataField.VehCountryField);
// var level = vehFinderApp.GetComboBoxOptions(DataField.VehLevelField);
// var type = vehFinderApp.GetComboBoxOptions(DataField.VehTypeField);


// test
// var repo = new DataManipulator(new VehiclesRepository
//     (new StringsTextualRepository()), new IteratorHelper());
// var y = repo.FindItem(new List<string> { "japan", "", "10" });
// foreach (var x in y)
// {
//     Console.WriteLine(x);
// }
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
        return _dataManipulator.FindUniqueItems(dataField);
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
    List<string> FindItem(string vehicleName, DataField searchField);
    List<string> FindUniqueItems(DataField dataField);
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

    public List<string> FindUniqueItems(DataField dataField)
    {
        var allUniqueValues = new HashSet<string>();

        foreach (var value in _vehicleCollection)
        {
            allUniqueValues.Add(value[(int)dataField]);
        }

        return allUniqueValues.ToList();
    }

    public List<string> FindItem(string vehicleName, DataField searchField)
    {
        return _iteratorHelper.Iterate(
            _vehicleCollection, new List<string> { vehicleName }, searchField);
    }

    public List<string> FindItem(List<string> searchParams)
    {
        var searchNamesList = new List<string>() { };

        var searchParameter = (int)SearchParams.NoParams;

        for (int i = 0; i < searchParams.Count; i++)
        {
            if (searchParams[i] == "") continue;
            searchParameter |= 1 << i;
            searchNamesList.Add(searchParams[i]);
        }

        var itemsFound = new List<string>();
        switch (searchParameter)
        {
            // Console.WriteLine("Show VehCountry and VehType");
            case (int)SearchParams.NoParams:
                return _iteratorHelper.Iterate(_vehicleCollection);
            case (int)SearchParams.Country:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehCountryField);
            case (int)SearchParams.Type:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehTypeField);
            case (int)SearchParams.Level:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehLevelField);
            case (int)SearchParams.CountryAndType:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehCountryField,
                    DataField.VehTypeField);
            case (int)SearchParams.CountryAndLevel:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehCountryField,
                    DataField.VehLevelField);
            case (int)SearchParams.TypeAndLevel:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehTypeField,
                    DataField.VehLevelField);
            case (int)SearchParams.CountryAndLevelAndType:
                return _iteratorHelper.Iterate(_vehicleCollection, searchNamesList, DataField.VehCountryField,
                    DataField.VehTypeField, DataField.VehLevelField);
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
        var paramsCount = itemName.Count;

        foreach (var item in collection)
        {
            switch (paramsCount)
            {
                case 1:
                    if (item[(int)dataField[0]].Contains(itemName[0]))
                    {
                        itemsFound.Add(string.Join(", ", item));
                    }

                    break;

                case 2:
                    if (item[(int)dataField[0]].Contains(itemName[0]) &&
                        item[(int)dataField[1]].Contains(itemName[1]))
                    {
                        itemsFound.Add(string.Join(", ", item));
                    }

                    break;

                case 3:
                    if (item[(int)dataField[0]].Contains(itemName[0]) &&
                        item[(int)dataField[1]].Contains(itemName[1]) &&
                        item[(int)dataField[2]].Contains(itemName[2]))
                    {
                        itemsFound.Add(string.Join(", ", item));
                    }

                    break;

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