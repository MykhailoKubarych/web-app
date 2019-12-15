
 const signUp = (email, password) => {
    let requestOptions = {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({email, password})
    };

    fetch('/api/auth/sign-up', requestOptions)
        .then(response => {
            if (!response.ok) {
                return Promise.reject({status: response.status, statusText: response.statusText});
            }
            return response.json();
        }).catch(failure => console.log(failure));
};

export const authService = {
    signOn: signUp
};  

