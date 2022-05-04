# IO.Swagger.Model.InsertStep
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**IdFrom** | **string** |  | [optional] 
**IdNext** | **List&lt;string&gt;** |  | [optional] 
**IdTo** | **string** |  | [optional] 
**DataToInsert** | [**List&lt;SimpleDataObject&gt;**](SimpleDataObject.md) | should be handled, outside see page 48, or use hash in calculation &#x3D;&gt; conflict have to be solved | [optional] 
**Handled** | **bool?** | both have to be update dataToInsert in their own set (\&quot;recursion anker\&quot;) | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

