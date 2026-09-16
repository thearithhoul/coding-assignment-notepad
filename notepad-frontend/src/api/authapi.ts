import { publicHttpClient } from './httpclient';


import type { loginRequestdto, tokenResponcedto, signInRequestdto, refreshTokenRequestdto } from './model/authdto';



const baseEndpoint = 'api/v1/auth';

async function loginApi(param: loginRequestdto): Promise<tokenResponcedto> {
    const { data } = await publicHttpClient.post<tokenResponcedto>(`${baseEndpoint}/login/user-password`, param);
    return data;
}

async function signinApi(param: signInRequestdto): Promise<tokenResponcedto> {
    const { data } = await publicHttpClient.post<tokenResponcedto>(`${baseEndpoint}/signin/user-password`, param);
    return data;
}

// refrash token
async function refrashtoken(param: refreshTokenRequestdto): Promise<tokenResponcedto> {
    const { data } = await publicHttpClient.post<tokenResponcedto>(`${baseEndpoint}/refrash`, param);
    return data;
}


export { loginApi, signinApi, refrashtoken };