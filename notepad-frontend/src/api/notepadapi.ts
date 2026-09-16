import { httpclient } from './httpclient';


import type {
    notedPadsRequestdto,
    notedPadsPagedResponcedto,
    notedPadResponcedto,
    notedPadsCreateRequestdto,
} from './model/notepaddto';



const baseEndpoint = 'api/v1/note';

async function getNotePadsApi(param: notedPadsRequestdto): Promise<notedPadsPagedResponcedto> {
    const { data } = await httpclient.get<notedPadsPagedResponcedto>(`${baseEndpoint}/list`, { params: param });
    return data;
}

async function getNotePadDetailApi(id: number): Promise<notedPadResponcedto> {
    const { data } = await httpclient.get<notedPadResponcedto>(`${baseEndpoint}/${id}`);
    return data;
}

async function createNotePadApi(param: notedPadsCreateRequestdto): Promise<notedPadResponcedto> {
    const { data } = await httpclient.post<notedPadResponcedto>(`${baseEndpoint}/create`, param);
    return data;
}

async function updateNotePadApi(id: number, param: notedPadsCreateRequestdto): Promise<notedPadResponcedto> {
    const { data } = await httpclient.post<notedPadResponcedto>(`${baseEndpoint}/update/${id}`, param);
    return data;
}

// Backend currently exposes this as GET (Controllers/NotepadController.cs), not DELETE.
async function removeNotePadApi(id: number): Promise<void> {
    await httpclient.get(`${baseEndpoint}/remove/${id}`);
}


export { getNotePadsApi, getNotePadDetailApi, createNotePadApi, updateNotePadApi, removeNotePadApi };
