# IO.Swagger.Api.SyncApi

All URIs are relative to *http://localhost:7042*

Method | HTTP request | Description
------------- | ------------- | -------------
[**SyncPost**](SyncApi.md#syncpost) | **POST** /sync | 
[**SyncPut**](SyncApi.md#syncput) | **PUT** /sync | 

<a name="syncpost"></a>
# **SyncPost**
> InlineResponse200 SyncPost (SyncBody1 body = null)



Checks if the fingerprint of own data matches the given fingerprint and optionally starts an asynchronous process to handle subset syncronization, if not

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class SyncPostExample
    {
        public void main()
        {
            var apiInstance = new SyncApi();
            var body = new SyncBody1(); // SyncBody1 |  (optional) 

            try
            {
                InlineResponse200 result = apiInstance.SyncPost(body);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling SyncApi.SyncPost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**SyncBody1**](SyncBody1.md)|  | [optional] 

### Return type

[**InlineResponse200**](InlineResponse200.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="syncput"></a>
# **SyncPut**
> InlineResponse200 SyncPut (SyncBody body = null)



Checks required actions for given list of sync steps

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class SyncPutExample
    {
        public void main()
        {
            var apiInstance = new SyncApi();
            var body = new SyncBody(); // SyncBody |  (optional) 

            try
            {
                InlineResponse200 result = apiInstance.SyncPut(body);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling SyncApi.SyncPut: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**SyncBody**](SyncBody.md)|  | [optional] 

### Return type

[**InlineResponse200**](InlineResponse200.md)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
