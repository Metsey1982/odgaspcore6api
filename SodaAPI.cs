using SODA;
using odgaspcore6api;

namespace odgaspcore6api
{
    public class SodaAPI
    {
       
       private SodaClient _sodaClient;
       private string _token = "kVpfLswf9QU2SKxIamKZwv65k";

       private string _name = "";

       private ResourceMetadata _metadata;
       private string _repositoryName = "";
       public string RepositoryName
       {
            get{return _repositoryName;}
       }
       private string _jsonData = "";
       public string JSONData
       {
            get{return _jsonData;}
       }

       private long _viewsCount = 0;
       public long ViewsCount
       {
            get{return _viewsCount;}
       }
      
       public SodaAPI(string url, string resourceIdentifier)
       {
            _sodaClient = new SodaClient(url,_token);
            _name = resourceIdentifier;
            _metadata = _sodaClient.GetMetadata(_name);
            _viewsCount = _metadata.ViewsCount;
            _repositoryName = _metadata.Name;
            //_setJSON();
       } 

    //    private void _setJSON()
    //    {
    //         var jsonData = _sodaClient.GetResource<PPPLoan2>(_name);
    //         _jsonData = jsonData.ToString();
    //    }
    //    public object GetJSON()
    //    {
    //         return _jsonData;
    //    }

    }
}