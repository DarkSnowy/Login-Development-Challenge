Vue.component('user-component', {
    props: {
        register: Boolean
    },
    data: function () {
        return {
            UserTitle: 'Register',
            Id: '',
            Firstname: '',
            Middlename: '',
            Lastname: '',
            Email: '',
            OldPassword: '',
            NewPassword: '',
            ConfirmPassword: '',
            Modified: '',
            Created: '',
            AwaitUserSubmit: false,
            Staff: false,
            Edit: false,
            Error: ''
        }
    },
    methods: {
        onSubmit() {
            // Get the vue component as the context of 'this' will change inside of the ajax post methods.
            var vue = this;

            if (!vue.Email) {
                vue.Error = "Email is missing";
                return;
            }

            if (!vue.NewPassword) {
                vue.Error = "Password is missing";
                return;
            }

            if (vue.NewPassword != vue.ConfirmPassword) {
                vue.Error = "Passwords do not match";
                return;
            }

            try {
                // v-show isn't working. Investigate later.
                vue.AwaitUserSubmit = true;

                // Set error as empty on new attempt.
                vue.Error = "";

                var path;

                if (vue.register)
                    path = "Users/Register";
                else
                    path = "Users"

                // Send login details to server.
                $.post("https://localhost:44363/" + path,
                    JSON.stringify({
                        "firstname": vue.Firstname,
                        "middlename": vue.Middlename,
                        "lastname": vue.Lastname,
                        "email": vue.Email,
                        "oldpassword": vue.OldPassword,
                        "password": vue.NewPassword
                    })
                ).fail(function (error) {
                    // If it failed then inform the user why. We set the returned message ourselves so we know it is safe for display.
                    vue.Error = error.responseJSON.message;
                }).done(function (data) {
                    if (vue.register) {
                        // Save the oauth token into a cookie for in case the page is refreshed.
                        $cookies.set('token', data.token, data.expiration);
                        // Add the token to the ajax header.
                        vue.$emit('login', data.token);
                    }
                }).always(function () {
                    // Always remove the loading spinner when call has completed.
                    vue.AwaitUserSubmit = false;
                });
            } catch(exception) {
                // If there is an error posting the request then don't leave the button loading forever.
                vue.AwaitUserSubmit = false;
                // Let the user know that something has gone wrong
                vue.Error = "Request failed";
            }
        },
        onCancel() {
            this.$emit('cancel', this.register)
        }
    },
    template: `
        <div class="row user-form">
            <div class="col-sm"></div>
            <div class="col-sm-6 mr-auto ml-auto form-wrapper">
                <div class="centre row" v-if="UserTitle != ''">
                    <div class="col-sm title" style="font-weight: 600; font-size: 20px;">
                        {{ UserTitle }}
                    </div>
                </div>
                <div class="centre row" v-if="Error != ''">
                    <div class="col-sm" style="color: red;">
                        {{ Error }}
                    </div>
                </div>
                <div class="row" v-if="Staff">
                    <div class="col-sm-4 fieldname">
                        Id
                    </div>
                    <div class="col-sm">
                        {{ Id }}
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Firstname
                    </div>
                    <div class="col-sm" v-if="!!Edit">
                        {{ Firstname }}
                    </div>
                    <div class="col-sm" v-if="Edit||register">
                        <input type="text" v-model="Firstname" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Middlename
                    </div>
                    <div class="col-sm" v-if="!!Edit">
                        {{ Middlename }}
                    </div>
                    <div class="col-sm" v-if="Edit||register">
                        <input type="text" v-model="Middlename" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Lastname
                    </div>
                    <div class="col-sm" v-if="!!Edit">
                        {{ Lastname }}
                    </div>
                    <div class="col-sm" v-if="Edit||register">
                        <input type="text" v-model="Lastname" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Email
                    </div>
                    <div class="col-sm" v-if="!!Edit">
                        {{ Email }}
                    </div>
                    <div class="col-sm" v-if="Edit||register">
                        <input type="text" v-model="Email" />
                    </div>
                </div>
                <div class="row" v-if="!register">
                    <div class="col-sm-4 fieldname">
                        Old Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" v-model="OldPassword" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        <span v-if="!register">New </span>Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" v-model="NewPassword" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Confirm Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" v-model="ConfirmPassword" />
                    </div>
                </div>
                <div class="row" v-if="Staff">
                    <div class="col-sm-4 fieldname">
                        Modified
                    </div>
                    <div class="col-sm">
                        {{ Modified }}
                    </div>
                </div>
                <div class="row" v-if="Staff">
                    <div class="col-sm-4 fieldname">
                        Created
                    </div>
                    <div class="col-sm">
                        {{ Created }}
                    </div>
                </div>
                <div class="row centre">
                    <div class="col-sm">
                        <button id="user-submit" :disabled="!!AwaitUserSubmit" v-on:click="onSubmit">
                            <i class="fa fa-spinner fa-spin" v-show="AwaitUserSubmit">&nbsp;&nbsp;</i>Submit
                        </button>
                    </div>
                </div>
                <div class="row centre">
                    <div class="col-sm">
                        <button v-on:click="onCancel">
                            Cancel
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-sm"></div>
        </div>`
});