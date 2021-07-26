import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChatdetailPage } from './chatdetail.page';

const routes: Routes = [
  {
    path: '',
    component: ChatdetailPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChatdetailPageRoutingModule {}
