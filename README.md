Just to explain what is going on:

I customized the default Login and Register pages by merging them into one view called LoginRegister.cshtml (under Account folder).
The view has two forms: one for Login form and another for the Register form and each of these has its own ValidationSummary.

When there is a server-side error such as invalid login and an error is added to the ModelState, the ValidationSummary of both will show the error.

The solution I imagined for this issue would be to have an additional taghelper on the validation summary something like *for-prefix* which would then only display errors for the ModelState with this prefix. In this example the value would be "loginFormModel" and "registerFormModel" respectively.

In general I like to seperate the model that is passed to the view (the ViewModel) from the object that is model-bound in the action parameter which I usually call the "FormModel".
I created a second version of the LoginRegister view that demonstrates how this is done and it is my hope that any development would also be applicable to this second scenario as well.

Thank you for taking the time to look into this.