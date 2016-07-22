namespace Microsoft.Azure.ApiHub.Table.Protocol
{
    internal static class Constants
    {
        public const string DataSetNameParameter = "datasetName";
        public const string TableNameParameter = "tableName";
        public const string ItemIdParameter = "itemId";

        public const string DataSetsMetadataTemplate = "$metadata.json/datasets";
        // TODO: Use the commented OData route once the Sql connector is fixed.
        public const string TableMetadataTemplate = "$metadata.json/datasets/{" + DataSetNameParameter + "}/tables/{" + TableNameParameter + "}";
        //public const string TableMetadataTemplate = "$metadata.json/datasets('{" + DataSetNameParameter + "}')/tables('{" + TableNameParameter + "}')";        
        public const string DataSetsTemplate = "datasets";
        public const string DataSetTemplate = "datasets('{" + DataSetNameParameter + "}')";
        public const string TablesTemplate = "datasets('{" + DataSetNameParameter + "}')/tables";
        public const string TableTemplate = "datasets('{" + DataSetNameParameter + "}')/tables('{" + TableNameParameter + "}')";
        public const string TableItemsTemplate = "datasets('{" + DataSetNameParameter + "}')/tables('{" + TableNameParameter + "}')/items";
        public const string TableItemTemplate = "datasets('{" + DataSetNameParameter + "}')/tables('{" + TableNameParameter + "}')/items('{" + ItemIdParameter + "}')";

        public const string DefaultDataSetName = "default";
    }
}
