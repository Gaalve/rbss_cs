# Org.OpenAPITools.Api.SyncApi

All URIs are relative to *http://localhost:7042*

Method | HTTP request | Description
------------- | ------------- | -------------
[**SyncPost**](SyncApi.md#syncpost) | **POST** /sync | 
[**SyncPut**](SyncApi.md#syncput) | **PUT** /sync | 


<a name="syncpost"></a>
# **SyncPost**
> InlineResponse200 SyncPost (InlineObject1? inlineObject1 = null)



Checks if the fingerprint of own data matches the given fingerprint and optionally starts an asynchronous process to handle subset syncronization, if not

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class SyncPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost:7042";
            var apiInstance = new SyncApi(config);
            var inlineObject1 = new InlineObject1?(); // InlineObject1? |  (optional) 

            try
            {
                InlineResponse200 result = apiInstance.SyncPost(inlineObject1);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SyncApi.SyncPost: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **inlineObject1** | [**InlineObject1?**](InlineObject1?.md)|  | [optional] 

### Return type

[**InlineResponse200**](InlineResponse200.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Returns information neighter an sync process is started or sync is done |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="syncput"></a>
# **SyncPut**
> InlineResponse200 SyncPut (InlineObject? inlineObject = null)



Checks required actions for given list of sync steps

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class SyncPutExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost:7042";
            var apiInstance = new SyncApi(config);
            var inlineObject = new InlineObject?(); // InlineObject? |  (optional) 

            try
            {
                InlineResponse200 result = apiInstance.SyncPut(inlineObject);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SyncApi.SyncPut: " + e.Message );
                Debug.Print("Status Code: "+ e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **inlineObject** | [**InlineObject?**](InlineObject?.md)|  | [optional] 

### Return type

[**InlineResponse200**](InlineResponse200.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Returns information neighter an sync process is started or sync is done |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

