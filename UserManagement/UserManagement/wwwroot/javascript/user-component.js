Vue.component('user-component', {
    props: {
        register: Boolean,
        staff: Boolean,
        id: String
    },
    data: function () {
        return {
            UserTitle: 'Register',
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
            edit: Boolean,
            Error: ''
        }
    },
    watch: {
        id: {
            immediate: true,
            handler: function (newVal, oldVal) {
                if (newVal != oldVal) {
                    if (newVal == 'me')
                        this.edit = true;

                    this.getUser(newVal);
                }
            }
        },
        register: {
            immediate: true,
            handler: function (register) {
                if (register) {
                    UserTitle = 'Register';
                } else {
                    UserTitle = 'Details';
                }
            }
        }
    },
    methods: {
        setFields(data) {
            this.Firstname = data.firstname;
            this.Middlename = data.middlename;
            this.Lastname = data.lastname;
            this.Email = data.email;
            this.Modified = data.modified;
            this.Created = data.created;
        },
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
                    path = "Users";

                // Send login details to server.
                $.post(apiUrl + path,
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
                        // Raise the login data to the parent.
                        vue.$emit('login', data);
                        // Get the current user details.
                        vue.getUser(vue.id);
                    } else {
                        vue.setFields(data)
                    }
                }).always(function () {
                    // Always remove the loading spinner when call has completed.
                    vue.AwaitUserSubmit = false;
                });
            } catch (exception) {
                // If there is an error posting the request then don't leave the button loading forever.
                vue.AwaitUserSubmit = false;
                // Let the user know that something has gone wrong
                vue.Error = "Request failed";
            }
        },
        onCancel() {
            this.$emit('cancel', this.register)
        },
        getUser(id) {
            // Get the vue component as the context of 'this' will change inside of the ajax post methods.
            var vue = this;

            $.get(apiUrl + "Users/" + id
            ).fail(function (error) {
                if (error.status && error.status == '401')
                    vue.Error = "Authorization failed";
                else
                    vue.Error = error.responseJSON.message;
            }).done(function (data) {
                vue.setFields(data)
            });
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
                <div class="row" v-if="staff">
                    <div class="col-sm-4 fieldname">
                        id
                    </div>
                    <div class="col-sm">
                        {{ id }}
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Firstname
                    </div>
                    <div class="col-sm" v-if="!edit">
                        {{ Firstname }}
                    </div>
                    <div class="col-sm" v-if="edit||register">
                        <input type="text" v-model="Firstname" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Middlename
                    </div>
                    <div class="col-sm" v-if="!edit">
                        {{ Middlename }}
                    </div>
                    <div class="col-sm" v-if="edit||register">
                        <input type="text" v-model="Middlename" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Lastname
                    </div>
                    <div class="col-sm" v-if="!edit">
                        {{ Lastname }}
                    </div>
                    <div class="col-sm" v-if="edit||register">
                        <input type="text" v-model="Lastname" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-sm-4 fieldname">
                        Email
                    </div>
                    <div class="col-sm" v-if="!edit">
                        {{ Email }}
                    </div>
                    <div class="col-sm" v-if="edit||register">
                        <input type="text" v-model="Email" />
                    </div>
                </div>
                <div class="row" v-if="id=='me'">
                    <div class="col-sm-4 fieldname">
                        Old Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" v-model="OldPassword" />
                    </div>
                </div>
                <div class="row" v-if="register||id=='me'">
                    <div class="col-sm-4 fieldname">
                        <span v-if="!register">New </span>Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" v-model="NewPassword" />
                    </div>
                </div>
                <div class="row" v-if="register||id=='me'">
                    <div class="col-sm-4 fieldname">
                        Confirm Password
                    </div>
                    <div class="col-sm middle">
                        <input type="password" v-model="ConfirmPassword" />
                    </div>
                </div>
                <div class="row" v-if="staff">
                    <div class="col-sm-4 fieldname">
                        Modified
                    </div>
                    <div class="col-sm">
                        {{ Modified }}
                    </div>
                </div>
                <div class="row" v-if="staff">
                    <div class="col-sm-4 fieldname">
                        Created
                    </div>
                    <div class="col-sm">
                        {{ Created }}
                    </div>
                </div>
                <div class="row centre" v-if="edit||register||id=='me'">
                    <div class="col-sm">
                        <button id="user-submit" :disabled="!!AwaitUserSubmit" v-on:click="onSubmit">
                            <i class="fa fa-spinner fa-spin" v-show="AwaitUserSubmit">&nbsp;&nbsp;</i>Submit
                        </button>
                    </div>
                </div>
                <div class="row centre" v-if="edit||register">
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