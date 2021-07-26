import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SiteauditPage } from './siteaudit.page';

const routes: Routes = [
  {
    path: '',
    component: SiteauditPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SiteauditPageRoutingModule {}
