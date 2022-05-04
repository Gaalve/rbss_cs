# Org.OpenAPITools.Api.ModifyApi

All URIs are relative to *http://localhost:7042*

Method | HTTP request | Description
------------- | ------------- | -------------
[**DeletePost**](ModifyApi.md#deletepost) | **POST** /delete | 
[**InsertPost**](ModifyApi.md#insertpost) | **POST** /insert | 
[**UpdatePost**](ModifyApi.md#updatepost) | **POST** /update | 


<a name="deletepost"></a>
# **DeletePost**
> void DeletePost (InlineObject4? inlineObject4 = null)



(Not to be implement yet) Deletes an existing data item into the data set and starts asynchronous synchronization with peer

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class DeletePostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost:7042";
            var apiInstance = new ModifyApi(config);
            var inlineObject4 = new InlineObject4?(); // InlineObject4? |  (optional) 

            try
            {
                apiInstance.DeletePost(inlineObject4);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ModifyApi.DeletePost: " + e.Message );
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
 **inlineObject4** | [**InlineObject4?**](InlineObject4?.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Returns information about success by deleting data |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="insertpost"></a>
# **InsertPost**
> void InsertPost (InlineObject2? inlineObject2 = null)



Inserts a new data item into the data set and starts asynchronous synchronization with peer

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class InsertPostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost:7042";
            var apiInstance = new ModifyApi(config);
            var inlineObject2 = new InlineObject2?(); // InlineObject2? |  (optional) 

            try
            {
                apiInstance.InsertPost(inlineObject2);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ModifyApi.InsertPost: " + e.Message );
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
 **inlineObject2** | [**InlineObject2?**](InlineObject2?.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Returns information about success by insterting data |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

<a name="updatepost"></a>
# **UpdatePost**
> void UpdatePost (InlineObject3? inlineObject3 = null)



(Not to be implement yet) Updates an existing data item into the data set and starts asynchronous synchronization with peer

### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Example
{
    public class UpdatePostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost:7042";
            var apiInstance = new ModifyApi(config);
            var inlineObject3 = new InlineObject3?(); // InlineObject3? |  (optional) 

            try
            {
                apiInstance.UpdatePost(inlineObject3);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ModifyApi.UpdatePost: " + e.Message );
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
 **inlineObject3** | [**InlineObject3?**](InlineObject3?.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | Returns information about success by updating data |  -  |

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)

