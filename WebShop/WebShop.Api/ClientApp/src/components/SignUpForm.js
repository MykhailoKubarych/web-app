import React, { useState } from "react";
import { authService } from '../_services';

function SignUpForm(props) {
    const [credentials, setCredentials] = useState(0);

    const submitHandler = e => {
        // if (credentials.email && credentials.password) {
        //     authService.signUp(credentials.email, credentials.password);
        //     console.log('signed up');WS
        //     return;
        // }
        // console.log('wrong credentials', credentials)
    }


    return (
        <div>
            <form onSubmit={submitHandler} >
                <label>
                    Email:
                    <input type='text' />
                    {/* <input type='text'onChange={e => setCredentials({email: e.target.value}) } /> */}
                </label>
                <label>
                    Password:
                    <input type='password' />
                    {/* <input type='password' onChange={e => setCredentials({password: e.target.value})} /> */}
                </label>
                <input type='submit' value='Submit' />
            </form>
        </div>
    );
};

export default SignUpForm;