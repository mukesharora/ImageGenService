TODO
o progress while saving

1. Impinj readers are launched by the schedule service. 
2. Schedule service reads CALCMan database.
3. Schedule service launches app.


Known issues:
1. If you put an invalid host name in a combo box, you may need to tab to it to change to enter a valid value.

Design decision
Configuration will be stored under the "MiddlewareConfigApp" application. It might have made sense 
to store some settings under CALCMan and Impinj, but things got complicated with some settings
being used by client and service, so in the end simplicity ruled.

