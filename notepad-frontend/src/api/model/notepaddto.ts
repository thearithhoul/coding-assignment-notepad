
interface notedPadsRequestdto {
    page: number,
    pageSize: number,
    search?: string,
    sorting?: number,
    filter?: string,
}

interface notedPadDetailResponcedto {
    id: number,
    content: string,
}

interface notedPadListResponcedto {
    id: number,
    title: string,
    subTitle: string,
    isPinned: boolean,
    isDeleted: boolean,
    createdAt: string,
    updatedAt: string,
}

interface notedPadResponcedto {
    id: number,
    title: string,
    subTitle: string,
    isPinned: boolean,
    isDeleted: boolean,
    createdAt: string,
    updatedAt: string,
    detail: notedPadDetailResponcedto,
}

interface notedPadsPagedResponcedto {
    items: notedPadListResponcedto[],
    totalCount: number,
}

interface notedPadsCreateDetailRequestdto {
    content: string,
}

interface notedPadsCreateRequestdto {
    title: string,
    subTitle: string,
    isPinned: boolean,
    detail: notedPadsCreateDetailRequestdto,
}

export type {
    notedPadsRequestdto,
    notedPadDetailResponcedto,
    notedPadListResponcedto,
    notedPadResponcedto,
    notedPadsPagedResponcedto,
    notedPadsCreateDetailRequestdto,
    notedPadsCreateRequestdto,
}
