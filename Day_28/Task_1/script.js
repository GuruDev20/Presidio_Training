const state = {
    callback: false,
    promise: false,
    async: false
};

function renderUsers(data, containerId) {
    const container = document.getElementById(containerId);
    container.innerHTML = data.map(user =>
        `<div class="user">#${user.id} - ${user.name} (${user.email})</div>`
    ).join('');
    container.classList.add('visible');
}

function clearUsers(containerId) {
    const container = document.getElementById(containerId);
    container.innerHTML = '';
    container.classList.remove('visible');
}

function toggleUsers(method) {
    const containerId = `${method}-users`;

    if (state[method]) {
        clearUsers(containerId);
        state[method] = false;
        return;
    }

    if (method === 'callback') {
        getUsersWithCallback((data) => {
            renderUsers(data, containerId);
        });
    } 
    else if (method === 'promise') {
        getUsersWithPromise()
        .then(data => renderUsers(data, containerId));
    } 
    else if (method === 'async') {
        getUsersAsyncAwait()
        .then(data => renderUsers(data, containerId));
    }
    state.callback = false;
    state.promise = false;
    state.async = false;

    state[method] = true;
}

function getUsersWithCallback(callback) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', 'user.json');
    xhr.onload = function () {
        if (xhr.status === 200) {
            const data = JSON.parse(xhr.responseText);
            callback(data);
        }
    };
    xhr.send();
}

function getUsersWithPromise() {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        xhr.open('GET', 'user.json');
        xhr.onload = function () {
            if (xhr.status === 200) {
                resolve(JSON.parse(xhr.responseText));
            } 
            else {
                reject('Failed to load');
            }
        };
        xhr.send();
    });
}

async function getUsersAsyncAwait() {
    const response = await fetch('user.json');
    const data = await response.json();
    return data;
}
