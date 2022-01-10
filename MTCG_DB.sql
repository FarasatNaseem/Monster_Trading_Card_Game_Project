-- Table: public.users

-- DROP TABLE IF EXISTS public.users;

CREATE TABLE IF NOT EXISTS public.users
(
    name character varying COLLATE pg_catalog."default" NOT NULL,
    token character varying COLLATE pg_catalog."default" NOT NULL,
    password character varying COLLATE pg_catalog."default" NOT NULL,
    coins integer,
    bio character varying COLLATE pg_catalog."default",
    image character varying COLLATE pg_catalog."default",
    elo integer,
    CONSTRAINT users_pkey PRIMARY KEY (name),
    CONSTRAINT users_token_key UNIQUE (token)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.users
    OWNER to root;

-- Table: public.package

-- DROP TABLE IF EXISTS public."package";

CREATE TABLE IF NOT EXISTS public."package"
(
    id integer NOT NULL DEFAULT nextval('package_id_seq'::regclass),
    "creatorToken" character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT package_pkey PRIMARY KEY (id),
    CONSTRAINT usertoken FOREIGN KEY ("creatorToken")
        REFERENCES public.users (token) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."package"
    OWNER to root;
-- Index: fki_t

-- DROP INDEX IF EXISTS public.fki_t;

CREATE INDEX IF NOT EXISTS fki_t
    ON public."package" USING btree
    ("creatorToken" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: fki_token

-- DROP INDEX IF EXISTS public.fki_token;

CREATE INDEX IF NOT EXISTS fki_token
    ON public."package" USING btree
    ("creatorToken" COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.packagecards

-- DROP TABLE IF EXISTS public.packagecards;

CREATE TABLE IF NOT EXISTS public.packagecards
(
    id character varying COLLATE pg_catalog."default" NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    damage character varying COLLATE pg_catalog."default" NOT NULL,
    pid integer NOT NULL,
    type character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT packagecards_pkey PRIMARY KEY (id),
    CONSTRAINT id FOREIGN KEY (pid)
        REFERENCES public."package" (id) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.packagecards
    OWNER to root;
-- Index: fki_id

-- DROP INDEX IF EXISTS public.fki_id;

CREATE INDEX IF NOT EXISTS fki_id
    ON public.packagecards USING btree
    (pid ASC NULLS LAST)
    TABLESPACE pg_default;


-- Table: public.usercards

-- DROP TABLE IF EXISTS public.usercards;

CREATE TABLE IF NOT EXISTS public.usercards
(
    id integer NOT NULL,
    usertoken character varying COLLATE pg_catalog."default" NOT NULL,
    cardid character varying COLLATE pg_catalog."default" NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    damage character varying COLLATE pg_catalog."default" NOT NULL,
    type character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT id PRIMARY KEY (id),
    CONSTRAINT usertoken FOREIGN KEY (usertoken)
        REFERENCES public.users (token) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.usercards
    OWNER to root;
-- Index: fki_f

-- DROP INDEX IF EXISTS public.fki_f;

CREATE INDEX IF NOT EXISTS fki_f
    ON public.usercards USING btree
    (usertoken COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Table: public.userdeck

-- DROP TABLE IF EXISTS public.userdeck;

CREATE TABLE IF NOT EXISTS public.userdeck
(
    id integer NOT NULL,
    usertoken character varying COLLATE pg_catalog."default" NOT NULL,
    cardid character varying COLLATE pg_catalog."default" NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    demage character varying COLLATE pg_catalog."default" NOT NULL,
    type character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT userdeck_pkey PRIMARY KEY (id),
    CONSTRAINT token FOREIGN KEY (usertoken)
        REFERENCES public.users (token) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.userdeck
    OWNER to root;

-- Table: public.userdeck

-- DROP TABLE IF EXISTS public.userdeck;

CREATE TABLE IF NOT EXISTS public.userdeck
(
    id integer NOT NULL,
    usertoken character varying COLLATE pg_catalog."default" NOT NULL,
    cardid character varying COLLATE pg_catalog."default" NOT NULL,
    name character varying COLLATE pg_catalog."default" NOT NULL,
    demage character varying COLLATE pg_catalog."default" NOT NULL,
    type character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT userdeck_pkey PRIMARY KEY (id),
    CONSTRAINT token FOREIGN KEY (usertoken)
        REFERENCES public.users (token) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.userdeck
    OWNER to root;


-- Table: public.sessionusers

-- DROP TABLE IF EXISTS public.sessionusers;

CREATE TABLE IF NOT EXISTS public.sessionusers
(
    usertoken character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT usertoken FOREIGN KEY (usertoken)
        REFERENCES public.users (token) MATCH SIMPLE
        ON UPDATE CASCADE
        ON DELETE CASCADE
        NOT VALID
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.sessionusers
    OWNER to root;
-- Index: fki_usertoken

-- DROP INDEX IF EXISTS public.fki_usertoken;

CREATE INDEX IF NOT EXISTS fki_usertoken
    ON public.sessionusers USING btree
    (usertoken COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;