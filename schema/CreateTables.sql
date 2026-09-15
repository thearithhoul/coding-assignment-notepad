CREATE TABLE app_user
(
    id                 INT GENERATED ALWAYS AS IDENTITY NOT NULL,
    user_name          VARCHAR(100)      NOT NULL,
    email              VARCHAR(255)      NOT NULL,
    password_hash      VARCHAR(255)      NULL,
    password_salt      VARCHAR(255)      NULL,
    first_name         VARCHAR(100)      NULL,
    last_name          VARCHAR(100)      NULL,
    phone_number       VARCHAR(20)       NULL,
    is_google_sign_in  BOOLEAN           NOT NULL DEFAULT FALSE,
    is_email_verified  BOOLEAN           NOT NULL DEFAULT FALSE,
    is_active           BOOLEAN          NOT NULL DEFAULT TRUE,
    failed_attempts     INT              NOT NULL DEFAULT 0,
    last_login_at       TIMESTAMPTZ      NULL,
    created_at          TIMESTAMPTZ      NOT NULL DEFAULT NOW(),
    updated_at          TIMESTAMPTZ      NOT NULL DEFAULT NOW(),
    deleted_at          TIMESTAMPTZ      NULL,

    CONSTRAINT PK_app_user PRIMARY KEY (id),
    CONSTRAINT UQ_app_user_user_name UNIQUE (user_name),
    CONSTRAINT UQ_app_user_email UNIQUE (email)
);

CREATE TABLE session
(
    id                     INT GENERATED ALWAYS AS IDENTITY NOT NULL,
    user_id                INT               NOT NULL,
    session_token          VARCHAR(500)      NOT NULL,
    refresh_token          VARCHAR(500)      NOT NULL,
    refresh_token_expiry   TIMESTAMPTZ       NOT NULL,
    created_at             TIMESTAMPTZ       NOT NULL DEFAULT NOW(),

    CONSTRAINT PK_session PRIMARY KEY (id),
    CONSTRAINT UQ_session_session_token UNIQUE (session_token),
    CONSTRAINT UQ_session_refresh_token UNIQUE (refresh_token),
    CONSTRAINT FK_session_app_user FOREIGN KEY (user_id)
        REFERENCES app_user (id) ON DELETE CASCADE
);

CREATE INDEX IX_session_user_id ON session (user_id);
