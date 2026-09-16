

interface loginRequestdto {
    username: string,
    password: string,

}

interface signInRequestdto {
    username: string,
    password: string,
    email: string,
    firstName: string,
    lastName: string,
    phoneNumber: string,

}


interface tokenResponcedto {
    accessToken: string,
    tokenType: string,
    refreshToken: string,
    expiresIn: number,
    expiresAt: string,
}

interface refreshTokenRequestdto {
    refreshToken: string,
}



export type { loginRequestdto, tokenResponcedto, signInRequestdto, refreshTokenRequestdto }