export interface LoginDto {
  username: string;
  password: string;
  rememberMe: boolean;
}

export interface AuthenticateResultModel {
  accessToken: string;
  encryptedAccessToken: string;
  expireInSeconds: number;
  userId: number;
}