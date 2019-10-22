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

Add json format request and response on route evalstr/json which only supports HttpPost
