Vue.component('login-component', {
    data: function () {
        return {
            Error: '',
            AwaitLogin: false,
            Email: '',
            Password: ''
        }
    },
    methods: {
        onSubmitLogin() {
            // Get the vue component as the context of 'this' will change inside of the ajax post methods.
            var vue = this;

            if (!vue.Email) {
                vue.Error = "Email is missing";
                return;
            }

            if (!vue.Password) {
                vue.Error = "Password is missing";
                return;
            }

            try {
                // v-show isn't working. Investigate later.
                vue.AwaitLogin = true;

                // Set error as empty on new attempt.
                vue.Error = "";

                // Send login details to server.
                $.post("https://localhost:44363/Users/Login",
                    JSON.stringify({ "email": vue.Email, "password": vue.Password })
                ).fail(function (error) {
                    // If it failed then inform the user why. We set the returned message ourselves so we know it is safe for display.
                    vue.Error = error.responseJSON.message;
                }).done(function (data) {
                    // Save the oauth token into a cookie for in case the page is refreshed.
                    $cookies.set('token', data.token, data.expiration);
                    // Add the token to the ajax header.
                    vue.$emit('login', data.token);
                }).always(function () {
                    // Always remove the loading spinner when call has completed.
                    vue.AwaitLogin = false;
                });
            } catch {
                // If there is an error posting the request then don't leave the button loading forever.
                vue.AwaitLogin = false;
                // Let the user know that something has gone wrong
                vue.Error = "Request failed";
            }
        },
        onNewRegister() {
            this.$emit('new');
        }
    },
    template: `
        <div class="row">
            <div class="col-sm"></div>
            <div class="col-sm mr-auto ml-auto form-wrapper">
                <div class="centre row">
                    <div class="col-sm title">
                        Login
                    </div>
                </div>
                <div class="centre row" v-if="Error != ''">
                    <div class="col-sm" style="color: red;">
                        {{ Error }}
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Email
                    </div>
                    <div class="col-sm middle">
                        <input type="text" id="login-email-input" v-model="Email" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" id="login-password-input" v-model="Password" />
                    </div>
                </div>
                <div class="centre row">
                    <div class="col-sm">
                        <button v-on:click="onSubmitLogin" :disabled="!!AwaitLogin">
                            <i class="fa fa-spinner fa-spin" v-show="AwaitLogin">&nbsp;&nbsp;</i>Submit
                        </button>
                    </div>
                </div>
                <div class="centre row">
                    <div class="col-sm">
                        <button v-on:click="onNewRegister">
                            Register
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-sm"></div>
        </div>`
})