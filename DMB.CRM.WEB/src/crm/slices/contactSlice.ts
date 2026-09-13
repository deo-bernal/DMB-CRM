import { createAsyncThunk, createSlice } from "@reduxjs/toolkit";
import http from "../services/http.service";
import type { Contact } from "../models";

export const fetchContacts = createAsyncThunk("contacts/list", async () => {
  const res = await http.get<Contact[]>("/contact/list");
  return res.data;
});

export const saveContact = createAsyncThunk(
  "contacts/save",
  async (payload: { id?: string; body: Partial<Contact> & { tagIds?: string[] } }) => {
    const res = payload.id
      ? await http.put<Contact>(`/contact/${payload.id}`, payload.body)
      : await http.post<Contact>("/contact", payload.body);
    return res.data;
  }
);

const slice = createSlice({
  name: "contacts",
  initialState: { items: [] as Contact[], loading: false },
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(fetchContacts.pending, (state) => {
        state.loading = true;
      })
      .addCase(fetchContacts.fulfilled, (state, action) => {
        state.loading = false;
        state.items = action.payload;
      })
      .addCase(fetchContacts.rejected, (state) => {
        state.loading = false;
      })
      .addCase(saveContact.fulfilled, (state, action) => {
        const idx = state.items.findIndex((c) => c.id === action.payload.id);
        if (idx >= 0) state.items[idx] = action.payload;
        else state.items.unshift(action.payload);
      });
  },
});

export default slice.reducer;
