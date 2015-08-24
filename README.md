#Football Live Score API 
API is offer Turkish Football League Data Including 
Live Score, Scorer Players, All Year Fixture and Updated Point Table

API collects data from www.tff.org and www.futbolingo.com with html scrapping method.

# Sample Requests


http://api.webron.social/TFF/GetScorers

returns Top Scorers as a JSON

http://api.webron.social/TFF/GetActualList 

returns actual point table of Turkish Football League as a JSON

http://api.webron.social/TFF/LiveScore/{statusParam}

returns live games results as a JSON if you send {statusParam} as 1 
and if there is not any live game you can send {statusParam} as a 0 to see some sample data

http://api.webron.social/TFF/GetFixture/{weekNo}

when you send weekNo as a parameter api returns related week game table

# hamdiceylan

www.hamdiceylan.com


