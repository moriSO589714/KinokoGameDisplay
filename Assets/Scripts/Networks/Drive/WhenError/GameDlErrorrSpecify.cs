using Google;
using Google.Apis.Sheets.v4.Data;
using System;
using System.IO;
using System.Net;

public class GameDlErrorrSpecify
{
    public GameDlCustomException SpecifyError(System.Exception e)
    {

        GoogleApiException googleAPIException = e as GoogleApiException;
        //GoogleAPI側のエラーであった場合
        if(googleAPIException != null)
        {
            string message = "インターネットエラーが発生しました";
            GameDlErrorType gameDlErrorType = GameDlErrorType.Unknown;
            switch (googleAPIException.HttpStatusCode) 
            {
                case HttpStatusCode.BadRequest:
                    message = "クライアント側のリクエストが不正です";
                    gameDlErrorType = GameDlErrorType.ImpossibleRecoveryErrorOnDrive;
                    break;
                case HttpStatusCode.Unauthorized:
                    message = "リクエストに無効な認証情報が含まれています";
                    gameDlErrorType = GameDlErrorType.ImpossibleRecoveryErrorOnDrive;
                    break;
                case HttpStatusCode.Forbidden:
                    message = "リクエストを実行する権限がユーザにありません";
                    gameDlErrorType = GameDlErrorType.ImpossibleRecoveryErrorOnDrive;
                    break;
                case HttpStatusCode.NotFound:
                    message = "リクエストされたページが見つかりません";
                    gameDlErrorType = GameDlErrorType.ImpossibleRecoveryErrorOnDrive;
                    break;
                case HttpStatusCode.TooManyRequests:
                    message = "APIへのリクエストが多すぎます";
                    gameDlErrorType = GameDlErrorType.NeedRetryAccessDrive;
                    break;
                case HttpStatusCode.InternalServerError:
                    message = "リクエストの処理中に予期しないエラーが発生しました";
                    gameDlErrorType = GameDlErrorType.NeedRetryAccessDrive;
                    break;
            }
            return new GameDlCustomException(message, e, gameDlErrorType);
        }

        InvalidDataException invalidDataException = e as InvalidDataException;
        //ファイルストリーム系のエラー(temp系のフォルダを全て削除してからダウンロードを行う)
        if(invalidDataException != null)
        {
            string message = "ファイルストリームエラー";
            GameDlErrorType gameDlErrorType = GameDlErrorType.NeedCleanDirectory;
            return new GameDlCustomException(message, e, gameDlErrorType);
        }

        IOException ioException = e as IOException;
        //IO系のエラー
        if(ioException != null)
        {
            string message = "IOエラー";
            GameDlErrorType gameDlErrorType = GameDlErrorType.NeedCleanDirectory;
            return new GameDlCustomException (message, e, gameDlErrorType);
        }

        return new GameDlCustomException("ダウンロード中に予期しないエラーが発生しました", e, GameDlErrorType.Others);
    }
}



