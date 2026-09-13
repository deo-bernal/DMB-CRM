import { configureStore } from "@reduxjs/toolkit";
import contactReducer from "../crm/slices/contactSlice";
import companyReducer from "../crm/slices/companySlice";
import tagReducer from "../crm/slices/tagSlice";
import pipelineReducer from "../crm/slices/pipelineSlice";

export const store = configureStore({
  reducer: {
    contacts: contactReducer,
    companies: companyReducer,
    tags: tagReducer,
    pipelines: pipelineReducer,
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
