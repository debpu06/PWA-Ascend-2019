namespace EPiServer.Reference.Commerce.Site.Features.Alloy.Data
{
    interface IFileDataImporter
    {
        string[] RetrieveAllData(string fullFileName);

        bool ImportFileExists(string fullFileName);

        string[] SplitByDelimiter(string row, char[] delimiter);
    }
}
