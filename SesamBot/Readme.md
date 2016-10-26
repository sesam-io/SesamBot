# SesamBot

SesamBot is a simple bot created using the [Microsoft Bot Framework](https://dev.botframework.com/) to query the Sesam API to get information about the Sesam node. 
The bot collects information from [Open Sesam](https://open.sesam.io), a Sesam installation open for the public, and the corresponding api endpoint at (https://open.sesam.io/api/api). 
Documentation for the Sesam api itself can be found [here](https://open.sesam.io/api/docs/api.html#api-reference).

The SesamBot uses [LUIS]()https://luis.ai) to interpret the messages, check out the [video](https://www.luis.ai/Help/)  to get a quick introduction.

The SesamBot can be run both on your local computer and in Azure. To run it locally it is recommended to install the [Bot Framework Channel Emulator](https://download.botframework.com/bf-v3/tools/emulator/publish.htm) to communicate with it.