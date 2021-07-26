import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ChataddPage } from './chatadd.page';

const routes: Routes = [
  {
    path: '',
    component: ChataddPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChataddPageRoutingModule {}
