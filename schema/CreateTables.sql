CREATE SCHEMA IF NOT EXISTS dbo;

CREATE EXTENSION IF NOT EXISTS pg_trgm;

CREATE TABLE dbo.app_user
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

CREATE TABLE dbo.session
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
        REFERENCES dbo.app_user (id) ON DELETE CASCADE
);

CREATE INDEX IX_session_user_id ON dbo.session (user_id);

CREATE TABLE dbo.noted_pads
(
    id                 INT GENERATED ALWAYS AS IDENTITY NOT NULL,
    user_id            INT               NOT NULL,
    title              VARCHAR(255)      NOT NULL,
    sub_title          VARCHAR(500)      NULL,
    is_pinned          BOOLEAN           NOT NULL DEFAULT FALSE,
    is_archived        BOOLEAN           NOT NULL DEFAULT FALSE,
    is_deleted         BOOLEAN           NOT NULL DEFAULT FALSE,
    created_at         TIMESTAMPTZ       NOT NULL DEFAULT NOW(),
    updated_at         TIMESTAMPTZ       NOT NULL DEFAULT NOW(),

    CONSTRAINT PK_noted_pads PRIMARY KEY (id),
    CONSTRAINT FK_noted_pads_app_user FOREIGN KEY (user_id)
        REFERENCES dbo.app_user (id) ON DELETE CASCADE
);

CREATE INDEX idx_noted_pads_title_trgm ON dbo.noted_pads USING GIN (title gin_trgm_ops);

CREATE TABLE dbo.noted_pad_detail
(
    id                 INT GENERATED ALWAYS AS IDENTITY NOT NULL,
    note_id            INT               NOT NULL,
    content            TEXT              NOT NULL,

    CONSTRAINT PK_noted_pad_detail PRIMARY KEY (id),
    CONSTRAINT FK_noted_pad_detail_noted_pads FOREIGN KEY (note_id)
        REFERENCES dbo.noted_pads (id) ON DELETE CASCADE
);

CREATE INDEX IX_noted_pad_detail_note_id ON dbo.noted_pad_detail (note_id);
