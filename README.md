# EvalStr
An web api project to evaluate the value of an expression which contains only non-negative integer numbers.

Route:
/evalstr

Query String Parameters:
(None)

Request Body:
The expression to be evaluated, e.g.: 4+5*2 (out: 14), 4+5/2 (out: 6.5), 4+5/2-1 (out: 5.5)

Request Type:
HttpGet/HttpPost

Response:
Status OK (200) + String Content (out value) when succeeded.
Status Bad Request (400) + String Content (error message) when failed.

11/10/2019: 

Add support for parentheses and negative number, example of expression: (4+5)*2+(-2) (out: 16)

22/10/2019:

Add json format request and response.

Route: 
/evalstr/json 

Request Type:
HttpPost

Input (ajax data): 
  Type: CalculatorRequest with only string property Expression, e.g.: {"Expression":"5*(+6*2 1)"}
  
Output:
  Type: CalculatorResponse with 3 properties.
  Property StatusCode - type: HttpStatusCode, please refer to the response of route /evalstr for details.
  Property Result - value of the expression when succeeded, type: string.
  Property ErrorMessage - error message when failed, type: string.
  Successful response example for input {"Expression":"5*(+6*2- 1)"}:
  {
    "statusCode": 200,
    "result": "55",
    "errorMessage": null
  }
  Failure response example for input {"Expression":"5*(+6*2 1)"}:
  {
    "statusCode": 400,
    "result": null,
    "errorMessage": "Operator is missing before character: 1 [index: 8]."
  }
