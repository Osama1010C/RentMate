namespace RentMateAPI.Helpers
{
    public static class DocumentHelper
    {
        public static List<string> ConvertDocumentToList(byte[] file)
        {
            var data = new List<string>();
            if (file == null) return data;

            using var memoryStream = new MemoryStream(file);
            using var reader = new StreamReader(memoryStream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                data.Add(line);
            }
            return data;
        }
    }
}
