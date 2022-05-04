# IO.Swagger.Api.ModifyApi

All URIs are relative to *http://localhost:7042*

Method | HTTP request | Description
------------- | ------------- | -------------
[**DeletePost**](ModifyApi.md#deletepost) | **POST** /delete | 
[**InsertPost**](ModifyApi.md#insertpost) | **POST** /insert | 
[**UpdatePost**](ModifyApi.md#updatepost) | **POST** /update | 

<a name="deletepost"></a>
# **DeletePost**
> void DeletePost (DeleteBody body = null)



(Not to be implement yet) Deletes an existing data item into the data set and starts asynchronous synchronization with peer

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class DeletePostExample
    {
        public void main()
        {
            var apiInstance = new ModifyApi();
            var body = new DeleteBody(); // DeleteBody |  (optional) 

            try
            {
                apiInstance.DeletePost(body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ModifyApi.DeletePost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**DeleteBody**](DeleteBody.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="insertpost"></a>
# **InsertPost**
> void InsertPost (InsertBody body = null)



Inserts a new data item into the data set and starts asynchronous synchronization with peer

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class InsertPostExample
    {
        public void main()
        {
            var apiInstance = new ModifyApi();
            var body = new InsertBody(); // InsertBody |  (optional) 

            try
            {
                apiInstance.InsertPost(body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ModifyApi.InsertPost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**InsertBody**](InsertBody.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatepost"></a>
# **UpdatePost**
> void UpdatePost (UpdateBody body = null)



(Not to be implement yet) Updates an existing data item into the data set and starts asynchronous synchronization with peer

### Example
```csharp
using System;
using System.Diagnostics;
using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace Example
{
    public class UpdatePostExample
    {
        public void main()
        {
            var apiInstance = new ModifyApi();
            var body = new UpdateBody(); // UpdateBody |  (optional) 

            try
            {
                apiInstance.UpdatePost(body);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling ModifyApi.UpdatePost: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**UpdateBody**](UpdateBody.md)|  | [optional] 

### Return type

void (empty response body)

### Authorization

No authorization required

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: Not defined

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
